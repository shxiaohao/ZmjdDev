webim 使用了Gulp对静态资源进行打包

gulp 安装配置过程：

1.安装Node.js cnpm gulp
gulp安装教程，使用教程，简单的自动化任务教程   http://jingyan.baidu.com/article/14bd256e7f7d7fbb6d2612c4.html

2.安装其它组件：
  cnpm install  gulp-minify-css --save-dev
  cnpm install --save-dev jshint gulp-jshint 
  cnpm install  gulp-uglify --save-dev
  cnpm install  gulp-concat --save-dev
  cnpm install  gulp-webserver --save-dev
  cnpm install  gulp-clean --save-dev
  cnpm install  gulp-minify-html --save-dev
  cnpm install  gulp-template --save-dev
   
 
3.编译
gulp build

4.自动编译


也可以在命令行工具中录入以下命令进行打包：
gulp build D:\Codes\WHotelDev\Dev\Site\WHotelWebsite\WHotelSite\WebIM\gulpfile.js
