var gulp   = require('gulp'),
    jshint = require('gulp-jshint'),
	uglify = require("gulp-uglify"),
	rename = require('gulp-rename'),
	beautify = require('gulp-beautify'),
	documentation = require('gulp-documentation'),
	concat = require('gulp-concat'),
	sources = [
    'src/bo/bo.js',
	'src/bo/ui/mediators/DlgMediator.js',
	'src/bo/ui/mediators/FileLoadMediator.js',
	'src/bo/ui/mediators/OverlayMediator.js',
	'src/bo/ui/views/DlgView.js',
	'src/bo/ui/views/FileLoadView.js',
	'src/bo/ui/views/OverlayView.js',
	'src/bo/ui/models/AppModel.js',
	'src/bo/ui/models/DataModel.js',
	'src/bo/ui/models/themes/AppDark.js',
	'src/bo/ui/models/themes/AppLight.js'
  ];

// define the default task and add the watch task to it
//gulp.task('default', ['watch']);

// configure the jshint task
gulp.task('jshint', function() {
	//var 
  
  return gulp.src(sources)
    .pipe(jshint())
    .pipe(jshint.reporter('jshint-stylish'));
});
gulp.task('minalljs', function () {
   // gulp.src('src/bo/**/*.js') // path to your files
   gulp.src(sources)
	//.pipe(concat('bo.min.js'))
    .pipe(uglify({mangle:false}))
	.pipe(rename({suffix: '.min'}))
    .pipe(gulp.dest('components/'));
});
gulp.task('minallone', function () {
    gulp.src('src/bo/**/*.js') // path to your files
	.pipe(concat('bo.min.js'))
    .pipe(uglify())
	//.pipe(rename({suffix: '.min'}))
    .pipe(gulp.dest('dist/bo/'));
});
gulp.task('minjs', function () {
	  
    gulp.src(sources)
    .pipe(concat('bo.min.js'))
    .pipe(uglify({mangle:false}))
	//.pipe(uglify({mangle:{except:['dispatcher', 'injector', 'datamodel']}}))
	//except:['instance', 'injector']
    .pipe(gulp.dest('./dist/bo'))
	.pipe(gulp.dest('./site/bo'))
});
gulp.task('beautify', function() {
  gulp.src('src/bo/**/*.js')
    .pipe(beautify({indentSize: 2}))
    .pipe(gulp.dest('beautify/bo/'));
});

gulp.task('doc', function () {
	 gulp.src('src/bo/**/*.js')
    .pipe(documentation({ format: 'html' }))
    .pipe(gulp.dest('docs/bo/html'));
});

//gulp.task('uncss', function() {
//  gulp.src('src/ap/**/*.js')
//.pipe(uncss({
 //           html: ['index.html', 'posts/**/*.html', 'http://example.com']
 //       }))
 
//});

gulp.task('build-js', function() {
  return gulp.src('src/bo/**/*.js')
    //.pipe(sourcemaps.init())
      .pipe(concat('bundle.js'))
      //only uglify if gulp is ran with '--type production'
      //.pipe(gutil.env.type === 'production' ? uglify() : gutil.noop()) 
   // .pipe(sourcemaps.write())
    .pipe(gulp.dest('beautify/test/'));
});

// configure which files to watch and what tasks to use on file changes
gulp.task('watch', function() {
  gulp.watch('source/javascript/**/*.js', ['jshint']);
});