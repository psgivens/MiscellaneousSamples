const path = require('path');
const merge = require('webpack-merge');
const webpack = require('webpack');
const NpmInstallPlugin = require('npm-install-webpack-plugin');

const TARGET = process.env.npm_lifecycle_event;
const PATHS = {
	app: path.join(__dirname, 'app'),
	build: path.join(__dirname, 'build')
};

const common = {
	entry: {
		app: PATHS.app
	},
    resolve: {
        extensions: ['', '.js', '.jsx']
    },
	output: {
		path: PATHS.build,
		filename: 'bundle.js'
	},
	module: {
		loaders: [
			{
				test: /\.css$/,
				loader: 'style!css',
				include: PATHS.app
			},
            {
                test: /\.jsx?$/,
                loader: 'babel?cacheDirectory',
                include: PATHS.app
            },
            // the url-loader uses DataUrls.
            // the file-loader emits files.
            { test: /\.(woff|woff2)$/,  loader: "url-loader?limit=10000&mimetype=application/font-woff" },
            { test: /\.ttf$/,    loader: "file-loader" },
            { test: /\.eot$/,    loader: "file-loader" },
            { test: /\.svg$/,    loader: "file-loader" },
						{ test: /\.wav$/,    loader: "url-loader?limit=8192&mimetype=audio/x-wav" },
            { test: /bootstrap-sass[\\\/]assets[\\\/]javascripts/, loader: "imports?jQuery=jquery" }
            //{ test: function(path){},
		]
	}
};

if (TARGET === 'start' || !TARGET) {
	module.exports = merge(common, {
		devTool: 'eval-source-map',
		devServer: {
			contentBase: PATHS.build,
			historyApiFallback: true,
			hot: true,
			inline: true,
			stats: 'errors-only',
			host: process.env.HOST,
			port: 8081 //process.env.PORT
		},
		plugins: [
			new webpack.HotModuleReplacementPlugin(),
			new NpmInstallPlugin({
				save: true // --save
			})
		]
	});
}

if (TARGET === 'build') {
	module.exports = merge(common, {});
}
