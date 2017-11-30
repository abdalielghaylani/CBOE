declare const __DEV__: boolean;
declare const __PRODUCTION__: boolean;
declare const __TEST__: boolean;

export const dev = __DEV__;
export const production = __PRODUCTION__;
export const test = __TEST__;
export const invIntegrationBasePath = '';
export const helpLinkBasePath = '/';
export const helpLink = `CBOEHelp/CBOEContextHelp/Registration%20Webhelp/default.htm`;
export const invNormalWindowParams =  `width=800,height=700`;
export const invWideWindowParams =  `width=1100,height=700`;
export const basePath = process.env.NODE_ENV === 'production' ? '/Registration.Server/' : '/';
export const fetchLimit = 20;
export const apiUrlPrefix = `${basePath}api/v1/`;
