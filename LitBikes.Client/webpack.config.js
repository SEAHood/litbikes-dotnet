var webpack = require('webpack');
const path = require('path');

module.exports = {
    entry: './src/game/game.ts',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                loader: 'ts-loader',
                exclude: /node_modules/
            }
        ]
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js']
    },
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, 'public/js')
    },
    plugins: [
        new webpack.ProvidePlugin({
            p5: 'p5'
        })
    ]
};