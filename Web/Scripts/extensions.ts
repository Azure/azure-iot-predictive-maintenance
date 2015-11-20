interface String {
    format(...value: any[]);
}

String.prototype.format = function () {
    var values = [];

    for (var i = 0; i < arguments.length; i++) {
        values[i - 0] = arguments[i];
    }

    var formatted = this;

    for (var i = 0; i < values.length; i++) {
        var regexp = new RegExp("\\{" + i + "\\}", "gi");
        if (values[i])
            formatted = formatted.replace(regexp, values[i]);
        else
            formatted = formatted.replace(regexp, "");
    }
    return formatted;
};