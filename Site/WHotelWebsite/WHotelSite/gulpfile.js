//引入 gulp
var gulp = require('gulp'); 

gulp.task('default', function() {
      // place code for your default task here
});

//引入插件
var gulpif = require('gulp-if');				//条件方法
var sass = require('gulp-sass');				//sass
var concat = require('gulp-concat');			//合并
var useref = require('gulp-useref');			//release html
var minifyHtml = require("gulp-minify-html");	//压缩html
var uglify = require('gulp-uglify'); 			//压缩js
var minifyCss = require('gulp-minify-css');		//压缩css
var imagemin = require('gulp-imagemin');		//压缩image
var pngquant = require('imagemin-pngquant');	
var stripDebug = require('gulp-strip-debug'); 	//该插件用来去掉console和debugger语句
var rename = require('gulp-rename');			//重命名
var template = require('gulp-template');		//模板使用

var version = '1.0';

//任务处理的文件路径配置
var staticPaths = {
	js_app_find : './Content/js/app/find.js',
	js_app_home: './Content/js/app/home.js',
	js_app_themeList: './Content/js/app/themeList.js',
	
	css_app_find: './Content/css/app/find.css',
	css_app_home: './Content/css/app/home.css',
	css_app_themeList: './Content/css/app/themeList.css',
	
	lib_js_jquery: './lib/js/jquery-1.10.2-min.js',
	lib_js_jquery_lazyload: './lib/js/jquery.lazyload.min.js',
	lib_js_zmjd: './lib/js/zmjd.js',
	lib_js_util: './lib/js/util.js',
	lib_js_amazeui: './lib/js/amazeui.min.js',
	lib_js_mui: './lib/js/mui.min.js',
	lib_js_vue_vue: './lib/js/vue/vue.min.js',
	
	lib_css_amazeui: './lib/css/amazeui.min.css',
	lib_css_mui: './lib/css/mui.min.css'
};

var paths = {
    js: [
        './Content/js/*'
    ],
    css: [
         './Content/css/*'
    ],
    sass: [
    	'./Content/scss/**/*.scss'
    ],
    img: [],
    
    lib: { 
        js: [],
        css: [],
        img: []
    },
    
    html: [
    	'./view/**/*.cshtml'
    ],
    
    app_find_js: [],
    app_find_css: []
};

//output path
var output = "./release/";  

/* 部署环境 */

gulp.task('htmls', function() {
	
    //html：app
    gulp.src(paths.html)
        //.pipe(minifyHtml())
        //.pipe(template({ v: version }))	//<%= v %>
        .pipe(useref())
        .pipe(gulpif('*.js', uglify()))
        .pipe(gulpif('*.css', minifyCss()))
        .pipe(gulp.dest(output + '/view/app'));
        
});

//压缩js
gulp.task('jsmin', function() {
	//js
    gulp.src(paths.js)
        .pipe(stripDebug())
        .pipe(uglify())
        .pipe(gulp.dest(output + '/js'));
        
	/*
	//app_find_js
    gulp.src(paths.app_find_js)
        .pipe(stripDebug())
        .pipe(concat('find.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest(output + '/js/app'));

	//app_find_css
    gulp.src(paths.app_find_css)
        .pipe(concat('find.min.css'))
        .pipe(minifyCss())
        .pipe(gulp.dest(output + '/css/app'));
        */
});

//编译Sass
gulp.task('sass', function() {
    gulp.src(paths.sass)
        .pipe(sass())
        .pipe(gulp.dest('./Content/css'));
});

//监听
gulp.task('watch', function() {  
  gulp.watch(paths.sass, ['sass']);  
}); 

//build default
gulp.task('build',  function() {
    gulp.start('htmls');
});
