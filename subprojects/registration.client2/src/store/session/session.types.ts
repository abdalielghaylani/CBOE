import {TypedRecord} from 'typed-immutable-record';

export interface IUser {
  fullName: string;
};

export interface IUserRecord extends TypedRecord<IUserRecord>, IUser {
};

export interface ISession {
  token: string;
  user: IUser;
  hasError: boolean;
  isLoading: boolean;
};

export interface ISessionRecord extends TypedRecord<ISessionRecord>,
  ISession {
};
