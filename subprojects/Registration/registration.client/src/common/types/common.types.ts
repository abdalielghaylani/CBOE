export interface INamedObject {
  name: string;
  description?: string;
}

export interface IShareableObject extends INamedObject {
  isPublic?: boolean;
}

export class CShareableObject implements IShareableObject {
  constructor(public name: string, public description?: string, public isPublic?: boolean) {
  }
}
