const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');
const WebpackConcatPlugin = require('webpack-concat-files-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const HtmlWebpackInjector = require('html-webpack-injector');
const HtmlWebpackTagsPlugin = require('html-webpack-tags-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require('css-minimizer-webpack-plugin');
const distPath = '../src/User Interface/Api/KiotVietTimeSheet.Api/wwwroot/';

module.exports = (_env, argv) => {
  var config = getConfig(
    argv.mode,
    {
      main: path.join(__dirname, 'src/index.js'),
    },
    'clocking-gps',
    'index.html'
  );
  var mobileConfig = getConfig(
    argv.mode,
    {
      main: path.join(__dirname, 'src/index-mobile.js'),
    },
    'mobile',
    'index-mobile.html'
  );
  setMobileConfig(mobileConfig);
  return [config, mobileConfig];
};

function getConfig(_mode, _entry, _output, _index) {
  const isDev = _mode === 'development';
  return {
    devtool: isDev ? 'eval-source-map' : false,
    entry: _entry,
    output: {
      clean: true,
      path: path.join(__dirname, distPath + _output),
      filename: isDev ? '[name].js' : 'main.[contenthash].js',
    },
    module: {
      rules: [
        {
          test: /^(?!.*\.(spec)\.js$).*\.js$/,
          include: /src/,
          exclude: /(node_modules|lib|vendor)/,
          use: [{ loader: 'babel-loader' }],
        },
        {
          test: /\.js$/,
          exclude: /(node_modules|bower_components|lib|vendor)/,
          use: {
            loader: 'babel-loader', //use babel-loader
            options: {
              presets: ['@babel/preset-env'], //Contains the modules converted from es6 to es5
            },
          },
        },
        {
          test: /\.html$/,
          use: [
            {
              loader: 'html-loader',
              options: {
                minimize: true,
              },
            },
          ],
        },
        {
          test: /\.(sa|sc|c)ss$/,
          use: [
            { loader: isDev ? 'style-loader' : MiniCssExtractPlugin.loader },
            { loader: 'css-loader', options: { sourceMap: false } },
            {
              loader: 'sass-loader',
              options: {
                sassOptions: {
                  sourceMap: false,
                  outputStyle: 'compressed',
                },
              },
            },
          ],
        },
        {
          test: /\.(svg|woff|woff2|ttf|eot|otf)([\?]?.*)$/,
          use: [
            {
              loader: 'file-loader?name=assets/fonts/[name].[ext]',
            },
          ],
        },
        // {
        //   test: /\.(png|jpg)$/,
        //   loader: "url-loader",
        //   options: {
        //     limit: 8192,
        //   },
        // },
      ],
    },
    stats: {
      colors: true,
    },
    devServer: {
      contentBase: path.join(__dirname, distPath),
      compress: true,
      port: 9292,
      disableHostCheck: true,
    },
    plugins: [
      new HtmlWebpackPlugin({
        template: _index,
      }),
      new MiniCssExtractPlugin({
        experimentalUseImportModule: true,
      }),
    ],
    optimization: {
      minimize: true,
      minimizer: [
        new TerserPlugin({
          terserOptions: {
            format: {
              comments: false,
            },
          },
          extractComments: false,
        }),
        new CssMinimizerPlugin({
          minimizerOptions: {
            preset: ['default', { discardComments: { removeAll: true } }],
          },
        }),
      ],
    },
  };
}
function setMobileConfig(config) {
  config.plugins[0].userOptions.inject = true;
  config.plugins[0].userOptions.chunks = 'all';
  //#region add plugins
  config.plugins.push(new HtmlWebpackInjector());
  config.plugins.push(
    new CopyPlugin({
      patterns: [
        {
          from: path.join(__dirname, 'favicon.ico'),
          to: path.join(__dirname, distPath),
        },
      ],
    })
  );
  config.plugins.push(
    new WebpackConcatPlugin({
      allowWatch: false,
      allowOptimization: true,
      bundles: [
        {
          dest: path.join(config.output.path, 'lib/vendor_head.js'),
          src: [
            path.join(__dirname, 'node_modules/jquery/dist/jquery.min.js'),
            path.join(__dirname, 'lib/bundles/angular.min.js'),
            path.join(__dirname, 'lib/bundles/angular-sanitize.min.js'),
            path.join(__dirname, 'lib/bundles/angular-route.min.js'),
            path.join(__dirname, 'lib/bundles/angular-ui-router.min.js'),
            path.join(
              __dirname,
              'node_modules/moment/min/moment-with-locales.min.js'
            ),
            path.join(__dirname, 'lib/ext/pulltorefresh.min.js'),
            path.join(__dirname, 'lib/ext/mobile-util.min.js'),
          ],
        },
        {
          dest: path.join(config.output.path, 'lib/kendo.js'),
          src: [
            path.join(__dirname, 'lib/bundles/kendo.all.min.js'),
            path.join(__dirname, 'lib/bundles/kendo.culture.vi-VN.min.js'),
          ],
        },
      ],
    })
  );
  config.plugins.push(
    new HtmlWebpackTagsPlugin({
      scripts: [
        {
          path: 'lib/vendor_head.js',
          hash: true,
          attributes: { defer: false },
        },
        {
          path: 'lib/kendo.js',
          hash: true,
        },
      ],
    })
  );
  //#endregion
}
