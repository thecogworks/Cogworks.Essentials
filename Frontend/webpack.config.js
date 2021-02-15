const webpack = require('webpack')
const findConfig = require('find-config')
const path = require('path')
const fs = require('fs')
const Dotenv = require('dotenv-webpack')
const HtmlWebPackPlugin = require('html-webpack-plugin')
const MiniCssExtractPlugin = require('mini-css-extract-plugin')
const CopyWebpackPlugin = require('copy-webpack-plugin')
const StyleLintPlugin = require('stylelint-webpack-plugin')
const CleanWebpackPlugin = require('clean-webpack-plugin')
const HtmlReplaceWebpackPlugin = require('html-replace-webpack-plugin')
const { VueLoaderPlugin } = require('vue-loader')
const glob = require('glob-all')
const PurgecssPlugin = require('purgecss-webpack-plugin')
const FixStyleOnlyEntriesPlugin = require('webpack-fix-style-only-entries')

const isDevelopment = process.env.NODE_ENV === 'development'
const isPublish = process.env.NODE_ENV === 'publish'
const isBuild = process.env.NODE_ENV === 'build'

module.exports = env => {
  let distPath
  if (isPublish) {
    distPath = findConfig.read('.publish')
  } else {
    distPath = 'dist'
  }

  const basePath = path.join(__dirname)

  const envfinalPath = isPublish ? basePath + '/.env.prod' : basePath + '/.env.dev'

  let entryPoints = {
    main: ['./src/js/polyfills/after-polyfill.js', './src/js/polyfills/closest-polyfill.js', './src/js/main.js']
  }

  if (!isDevelopment) {
    entryPoints = {
      ...entryPoints,
      backoffice: ['./src/scss/backoffice.scss']
    }
  }

  let webpackConfiguration = {
    mode: (isDevelopment) ? 'development' : 'production',
    entry: entryPoints,
    output: {
      path: path.join(__dirname, distPath),
      chunkFilename: 'js/[name].js',
      filename: 'js/[name].js'
    },
    optimization: {
      runtimeChunk: 'single',
      splitChunks: {
        cacheGroups: {
          vendors: {
            test: /[\\/]node_modules[\\/]/,
            name: 'libs',
            enforce: true,
            chunks: 'all'
          }
        }
      }
    },
    resolve: {
      extensions: ['.js']
    },
    module: {
      rules: [
        {
          test: /\.html$/,
          use: [
            {
              loader: 'html-loader',
              options: {
                interpolate: true
              }
            }
          ]
        },
        {
          enforce: 'pre',
          test: /\.(js|vue)$/,
          exclude: /node_modules/,
          loader: 'eslint-loader',
          options: {
            formatter: require('eslint-friendly-formatter')
          }
        },
        {
          test: /\.js$/,
          exclude: /node_modules\/(?!(swiper|dom7)\/).*/,
          loader: 'babel-loader'
        },
        {
          test: /\.vue$/,
          use: 'vue-loader'
        },
        {
          test: /\.(png|jpe?g|svg|gif|webp)/i,
          use: [
            {
              loader: 'url-loader',
              options: {
                name: '[path][name].[ext]',
                limit: 50,
                outputPath: 'images',
                publicPath: '../images/',
                context: 'src/images'
              }
            },
            {
              loader: 'img-loader',
              options: {
                name: '[path][name].[ext]',
                useRelativePath: true,
                context: 'src/images'
              }
            }
          ]
        },
        {
          test: /\.json/i,
          use: [
            {
              loader: 'url-loader',
              options: {
                name: '[path][name].json',
                limit: 50,
                outputPath: 'json',
                publicPath: '../json/',
                context: 'src/json'
              }
            }
          ]
        },
        {
          test: /\.scss$/,
          use: [
            MiniCssExtractPlugin.loader,
            {
              loader: 'css-loader',
              options: {
                minimize: true,
                outputPath: 'css'
              }
            },
            'postcss-loader',
            'sass-loader'
          ]
        },
        {
          test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
          exclude: [
            path.resolve(__dirname, 'src/images')
          ],
          use: [{
            loader: 'file-loader',
            options: {
              name: '[name].[ext]',
              outputPath: 'fonts/',
              publicPath: '../fonts'
            }
          }]
        }
      ]
    },
    plugins: [
      new Dotenv({
        path: envfinalPath
      }),
      new VueLoaderPlugin(),
      new FixStyleOnlyEntriesPlugin(),
      new MiniCssExtractPlugin({
        filename: 'css/[name].css',
        chunkFilename: 'css/[name].css'
      }),
      new CopyWebpackPlugin([
        {
          from: 'src/fonts', to: 'fonts', ignore: ['selection.json', '.gitignore']
        },
        {
          from: 'src/images', to: 'images', ignore: (isPublish) ? ['static/*', '.gitignore'] : []
        },
        {
          from: 'src/json', to: 'json', ignore: (isPublish) ? ['*.*', '.gitignore'] : []
        }
      ]),
      new StyleLintPlugin({
        extends: 'stylelint-config-sass-guidelines',
        configFile: '.stylelintrc',
        context: './src/scss',
        files: '**/*.scss',
        failOnError: false,
        quiet: false
      }),
      new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/)
    ],
    stats: {
      children: false
    },
    performance: {
      hints: false
    }
  }

  webpackConfiguration.plugins.push(
    new PurgecssPlugin({
      paths: glob.sync([
        './src/templates/**/*.html',
        './src/js/**/*.vue',
        './src/js/**/*.js',
        './node_modules/vue-custom-scrollbar/**/*.{css,js}'
      ]),
      whitelistPatterns: [/^ps/]
    })
  )

  if (!isPublish) {
    // clean dist directory
    webpackConfiguration.plugins.push(new CleanWebpackPlugin([distPath + '/*']))

    // Get available html templates from /src/templates directory
    let templates = fs.readdirSync('src/templates').filter(file => (path.extname(file) === '.html'))
    templates.map(page => {
      if (page !== 'backoffice.html') {
        webpackConfiguration.plugins.push(new HtmlWebPackPlugin({
          template: 'src/templates/' + page,
          filename: page,
          excludeChunks: ['backoffice'],
          hash: true
        }))
      } else {
        webpackConfiguration.plugins.push(new HtmlWebPackPlugin({
          template: 'src/templates/' + page,
          filename: page,
          hash: true
        }))
      }
    })

    if (isDevelopment) {
      webpackConfiguration.devServer = {
        setup (app) {
          app.post('*', (req, res) => {
            res.redirect(req.originalUrl)
          })
        }
      }
    }

    // Overwrite paths to images in HTML templates
    webpackConfiguration.plugins.push(
      new HtmlReplaceWebpackPlugin([
        {
          pattern: '../../images',
          replacement: 'images'
        },
        {
          pattern: '../images',
          replacement: 'images'
        }
      ])
    )
  }

  if (!isBuild && !isPublish) {
    if (env && env.useProxy && env.proxyUrl && env.proxyUrl !== '') {
      webpackConfiguration.devServer =
        {
          index: '',
          compress: true,
          open: true,
          overlay: {
            warnings: false,
            errors: true
          },
          proxy: {
            '**': {
              target: env.proxyUrl,
              changeOrigin: true
            }
          }
        }
    }
  }

  return webpackConfiguration
}
