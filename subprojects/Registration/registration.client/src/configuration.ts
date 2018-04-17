declare const __DEV__: boolean;
declare const __PRODUCTION__: boolean;
declare const __TEST__: boolean;

export const dev = __DEV__;
export const production = __PRODUCTION__;
export const test = __TEST__;
export const invIntegrationBasePath = '';
export const helpLinkUserGuide = `CBOEHelp/Web Registration User Guide/Web Registration User guide.htm`;
export const helpLinkAdminGuide = `CBOEHelp/Web Registration Admin Guide/Registration Administrator Guide.htm`;
export const invNormalWindowParams =  `width=800,height=700`;
export const invWideWindowParams =  `width=1100,height=700`;
export const basePath = process.env.NODE_ENV === 'production' ? '/Registration.Server/' : '/';
export const fetchLimit = 200;
export const printAndExportLimit = 1000;
export const apiUrlPrefix = `${basePath}api/v1/`;
