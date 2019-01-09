/// <binding AfterBuild='default' />
"use strict";

var gulp = require('gulp'),
	babel = require('gulp-babel'),
	filter = require('gulp-filter'),
	zip = require('gulp-zip');

var del = require('del'),
	vinylPaths = require('vinyl-paths'),
	webpack_stream = require('webpack-stream');

//var webpackConfig = require('./VaultApplication/UI/webpack.config.js');

//gulp.task('clean', () => {
//	return gulp.src(['./VaultApplication/UI/RetentionTable/build/**/*'])
//		.pipe(vinylPaths(del));
//});

//gulp.task('ReactUI', ['clean'], () => {
//	return webpack_stream(webpackConfig)
//		.pipe(gulp.dest('./VaultApplication/UI/RetentionTable/build/'));
//});

gulp.task('prepareBin', () => {
	return gulp.src('./src/ScriptCs.Embedded/bin/Debug/**/*')
		.pipe(gulp.dest('./src/ScriptCs.Embedded/UI/bin'));
});

gulp.task("PackageUI", ['prepareBin'], () => {
	// For now we are just filtering it to one file, this will be what gets transformed with babel
	const jsFileFilter = filter([
		'**/Application.js'
	], { restore: true });

	return gulp.src([
		'./src/ScriptCs.Embedded/UI/**',
		//'./ScriptCs.Embedded/UI/appdef.xml',
		//'./ScriptCs.Embedded/UI/Application.js',
		//'./ScriptCs.Embedded/UI/ScriptInterface.html'
	])
		.pipe(jsFileFilter) // Filter the files for babel to process
		.pipe(babel({
			presets: ['env']
		}))
		.pipe(jsFileFilter.restore) // Restore the files so everything gets put in the zip (this merges in the babel processed files with the existing files)
		.pipe(zip('UI.mfappx'))
		.pipe(gulp.dest("./src/ScriptCs.Embedded/bin"));
});

gulp.task('package', ["PackageUI"]);

gulp.task('default', ["package"]);
