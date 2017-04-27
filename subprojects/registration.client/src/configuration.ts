declare const __DEV__: boolean;
declare const __PRODUCTION__: boolean;
declare const __TEST__: boolean;

export const dev = __DEV__;
export const production = __PRODUCTION__;
export const test = __TEST__;
export const basePath = process.env.NODE_ENV === 'production' ? '/Registration.Server/' : '/';
export const fetchLimit = 1000;
export const apiUrlPrefix = `${basePath}api/v1/`;
