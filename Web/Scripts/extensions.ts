String.prototype.format = function () {
    var values = [];
    for (var _i = 0; _i < arguments.length; _i++) {
        values[_i - 0] = arguments[_i];
    }
    var formatted = this;
    for (var i = 0; i < values.length; i++) {
        var regexp = new RegExp('\\{' + i + '\\}', 'gi');
        if (values[i])
            formatted = formatted.replace(regexp, values[i]);
        else
            formatted = formatted.replace(regexp, '');
    }
    return formatted;
};