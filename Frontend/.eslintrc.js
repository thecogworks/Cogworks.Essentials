module.exports = {
    "env": {
        "browser": false,
        "es6": true
    },
    "parserOptions": {
        "ecmaVersion": 2018,
        "ecmaFeatures": {
          "arrowFunctions": true,
          "blockBindings": true,
          "classes": true,
          "defaultParams": true,
          "modules": true,
          "spread": true,
          "globalReturn": true
        },
        "sourceType": "module"
    },
    "extends": [
        "standard",
        "plugin:vue/recommended"
    ],
    "rules": {
        "vue/no-v-html": "off"
    }
};