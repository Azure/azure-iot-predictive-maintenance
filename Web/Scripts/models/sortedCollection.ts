module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class SortedCollection<T>  {
        private collection: KnockoutObservableArray<T>;

        public sortBy: KnockoutObservable<string>;
        public sortAccending: KnockoutObservable<boolean>;

        constructor(collection: KnockoutObservableArray<T>) {
            this.collection = collection;
            this.observable = this.observable.bind(this);

            this.sortBy = ko.observable<string>();
            this.sortAccending = ko.observable<boolean>(true);
        }

        observable(): KnockoutComputed<T> {
            return (<any>_).sortByOrder(this.collection(), this.sortBy(), this.sortAccending());
        }
    }
}