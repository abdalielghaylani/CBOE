export class Project {
    constructor(
        public id: string,
        public name: string,
        public description: string,
        public type: string,
        public isActive: boolean,
        public isPublic: boolean
    ) {};
}
