/// <binding />
var gulp = require('gulp');
var typescript = require('gulp-tsc');
var concat = require('gulp-concat');
var clean = require('gulp-clean');
var runSequence = require('run-sequence');
var order = require("gulp-order");
var debug = require('gulp-debug');
var minify = require('gulp-minify');

var SRC_ROOT = './Client',
    TS_SRC = [
        SRC_ROOT + '/references.ts',
        SRC_ROOT + '/game/game.ts',
        SRC_ROOT + '/dto.ts',
        SRC_ROOT + '/util.ts',
        SRC_ROOT + '/model/arena.ts',
        SRC_ROOT + '/model/bike.ts',
    ],

    COMPILED_DIR = './wwwroot/lib/litbikes',
    COMPILED_FILES = COMPILED_DIR + '/**/*',

    JS_ORDER = [
        '*.js',
        'model/*.js',
        'game/game.js',
    ],

    DEPLOY_JS_NAME = 'litbikes.js'
    ;

gulp.task('build+deploy', function (callback) {
    return runSequence(
        'clean',
        'build:ts'
    );
});


var paths = {
    scripts: ['test/**/*.js', 'test/**/*.ts', 'test/**/*.map'],
};

gulp.task('default', function () {
    gulp.src(paths.scripts).pipe(gulp.dest('wwwroot/scripts'))
});

gulp.task('clean', function () {
    return gulp.src([COMPILED_FILES], { read: false, allowEmpty: true })
        .pipe(clean({ force: true }))
});

gulp.task('build:ts', function () {
    return gulp.src('./src/game.ts')
        .pipe(typescript({
            module: "commonjs",
            target: 'es5'
        }))
        .pipe(order(JS_ORDER))
        .pipe(concat(DEPLOY_JS_NAME))
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            noSource: true
        }))
        .pipe(gulp.dest('./public/js'))
});