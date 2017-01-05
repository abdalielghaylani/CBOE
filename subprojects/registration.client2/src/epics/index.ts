import { ConfigurationEpics } from './configuration.epics';
import { SessionEpics } from './session.epics';

export const EPIC_PROVIDERS = [ ConfigurationEpics, SessionEpics ];
export { ConfigurationEpics, SessionEpics };
