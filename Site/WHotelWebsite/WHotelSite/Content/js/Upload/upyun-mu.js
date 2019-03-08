'use strict';
(function() {
    var _config = {
        api: 'http://m0.api.upyun.com/',
        chunkSize: 1048576
    };

    var _failPicArray = [];

    function _extend(dst, src) {
        for (var i in src) {
            dst[i] = src[i];
        }
    }

    function sortPropertiesByKey(obj) {
        var keys = [];
        var sorted_obj = {};
        for (var key in obj) {
            if (obj.hasOwnProperty(key)) {
                keys.push(key);
            }
        }
        keys.sort();
        for (var i = 0; i < keys.length; i++) {
            var k = keys[i];
            sorted_obj[k] = obj[k];
        }
        return sorted_obj;
    }

    function calcSign(data, secret) {
        if (typeof data !== 'object') {
            return;
        }
        var sortedData = sortPropertiesByKey(data);
        var md5Str = '';
        for (var key in sortedData) {
            if (key !== 'signature') {
                md5Str = md5Str + key + sortedData[key];
            }
        }
        var sign = SparkMD5.hash(md5Str + secret);
        return sign;
    }

    function formatParams(data) {
        var arr = [];
        for (var name in data) {
            arr.push(encodeURIComponent(name) + "=" + encodeURIComponent(data[name]));
        }
        return arr.join("&");
    }

    function checkBlocks(arr) {
        var indices = [];
        var idx = arr.indexOf(0);
        while (idx != -1) {
            indices.push(idx);
            idx = arr.indexOf(0, idx + 1);
        }
        return indices;
    }
    
    function upload(path, file, catID, index) {
        var self = this;
        async.waterfall([
            function(callback) {
                var chunkInfo = {
                    chunksHash: {}
                };
                if (!file) {
                    console.log('no file is selected');
                    return;
                }
                var blobSlice = File.prototype.slice || File.prototype.mozSlice || File.prototype.webkitSlice;
                var chunkSize = _config.chunkSize;
                var chunks = Math.ceil(file.size / chunkSize);
                var currentChunk = 0;
                var spark = new SparkMD5.ArrayBuffer();
                var frOnload = function(e) {
                    chunkInfo.chunksHash[currentChunk] = SparkMD5.ArrayBuffer.hash(e.target.result);
                    spark.append(e.target.result);
                    currentChunk++;
                    if (currentChunk < chunks) {
                        loadNext();
                    } else {
                        chunkInfo.entire = spark.end();
                        chunkInfo.chunksNum = chunks;
                        chunkInfo.file_size = file.size;
                        callback(null, chunkInfo);
                        return;
                    }
                };
                var frOnerror = function() {
                    console.warn("oops, something went wrong.");
                };

                function loadNext() {
                    var fileReader = new FileReader();
                    fileReader.onload = frOnload;
                    fileReader.onerror = frOnerror;
                    var start = currentChunk * chunkSize,
                        end = ((start + chunkSize) >= file.size) ? file.size : start + chunkSize;
                    var blobPacket = blobSlice.call(file, start, end);
                    fileReader.readAsArrayBuffer(blobPacket);
                }
                loadNext();
            },
            function(chunkInfo, callback) {
                var options = {
                    'path': path,
                    'expiration': _config.expiration,
                    'file_blocks': chunkInfo.chunksNum,
                    'file_size': chunkInfo.file_size,
                    'file_hash': chunkInfo.entire
                };

                _extend(options, self.options);

                if (self._signature) {
                    var signature = self._signature;
                } else {
                    var signature = calcSign(options, _config.form_api_secret);
                }
                var policy = btoa(JSON.stringify(options));
                var paramsData = {
                    policy: policy,
                    signature: signature
                };
                var urlencParams = formatParams(paramsData);
                var request = new XMLHttpRequest();
                request.open('POST', _config.api + _config.bucket + '/');
                request.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                request.onload = function(e) {
                    if (request.status == 200) {
                        callback(null, chunkInfo, request.response);
                    } else {
                        request.send(urlencParams);
                    }
                };
                request.send(urlencParams);
            },
            function(chunkInfo, res, callback) {
                res = JSON.parse(res);
                var chunkSize = _config.chunkSize;
                var _status = res.status;
                var result;
                async.until(function() {
                    return checkBlocks(_status).length <= 0;
                }, function(callback) {
                    var idx = checkBlocks(_status)[0];
                    var start = idx * chunkSize,
                        end = ((start + chunkSize) >= file.size) ? file.size : start + chunkSize;
                    var packet = file.slice(start, end);

                    var options = {
                        'save_token': res.save_token,
                        'expiration': _config.expiration,
                        'block_index': idx,
                        'block_hash': chunkInfo.chunksHash[idx]
                    };

                    var signature = calcSign(options, res.token_secret);
                    var policy = btoa(JSON.stringify(options));

                    var formDataPart = new FormData();
                    formDataPart.append('policy', policy);
                    formDataPart.append('signature', signature);
                    formDataPart.append('file', packet);

                    var request = new XMLHttpRequest();
                    request.onreadystatechange = function(e) {
                        if (e.currentTarget.readyState === 4 && e.currentTarget.status == 200) {
                            _status = JSON.parse(e.currentTarget.response).status;
                            result = request.response;
                            callback(null);
                        }
                    };
                    request.open('POST', _config.api + _config.bucket + '/');//wwb 0507
                    request.send(formDataPart);

                }, function(err) {
                    if (err) {
                        callback(err);
                    }
                    callback(null, chunkInfo, result);
                });
            },
            function(chunkInfo, res, callback) {
                res = JSON.parse(res);

                var options = {
                    'save_token': res.save_token,
                    'expiration': _config.expiration
                };

                var signature = calcSign(options, res.token_secret);
                var policy = btoa(JSON.stringify(options));
                var formParams = {
                    policy: policy,
                    signature: signature
                };
                var formParamsUrlenc = formatParams(formParams);
                var request = new XMLHttpRequest();
                request.open('POST', _config.api + _config.bucket + '/');
                request.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                request.onload = function(e) {
                    if (request.status == 200) {
                        callback(null, request.response);
                    } else {
                        callback(null, request.response);
                    }
                };
                request.send(formParamsUrlenc);
            }
        ], function (err, response) {
            if (err) {
                var temp = { path: path, picCatID: catID, name: file.name, index: index };
                _failPicArray.push(temp);
            }
            else {
                var res = JSON.parse(response);
                res.contentsecret = self.options["content-secret"];
                res.commentID = self.options["commentID"];
                res.TagBlockCategory = catID;
                self.picCount--;                
                
                res.picsCount = self.picCount;
                res._failPicArray = _failPicArray;

                var picPath = res.path;
                var bucket_name = res.bucket_name;
                var image_width = res.image_width;
                var image_height = res.image_height;
                var mimetype = res.mimetype;
                var imageType = mimetype.split('/')[1];
                var file_size = res.file_size;
                var photoSecret = res.contentsecret;
                var commentID = res.commentID;
                var blockTagCatID = res.TagBlockCategory;

                insertNewFile(commentID, blockTagCatID, picPath, photoSecret, imageType, file_size, image_width, image_height, file.name, catID);

                if (res.picsCount <= 0) {
                    console.log("Upanyun all pics done,waiting insertfile complete....." + new Date().toGMTString);
                }

                //if (!judgeIE()) {
                //    var myEvent = new CustomEvent('uploadfinish', {
                //        'detail': res
                //    });
                //    document.dispatchEvent(myEvent);
                //}
                //else{

                //}
                /*异步waterfalll 按顺序执行完毕后 必须执行此步调用*/
            }
        });
    }

    function Sand(config) {
        _extend(_config, config);

        if(config.signature) {
            this._signature = config.signature;
        }

        this.setOptions = function(options) {
            this.options = options;
        };

        this.setPicCount = function (picCount) {
            if (typeof picCount == 'number' && picCount > 0) {
                this.picCount = picCount;
            }
        };

        this.upload = upload;
    }

    // bind the construct fn. to global
    this.Sand = Sand;

}).call(this);