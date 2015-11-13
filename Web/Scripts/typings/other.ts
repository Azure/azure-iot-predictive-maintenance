declare var powerbi: any;

interface JQueryStatic {
    connection: any;
}

interface String {
    contains(value: string, caseInsensitive?: boolean): boolean;
    containsOneOf(substrings: Array<string>): boolean;
    format(...value: any[]);
}