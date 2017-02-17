import { IAppState } from '../../store';

export enum GroupSettingType {
  Registration,
  DuplicateChecking,
  Search,
  Inventory,
  Advanced,
  ExtendedDuplicateChecking
}

export class CSetting {
  name?: String;
  value?: String;
  description?: String;
  allowedValues?: String;
  controlType?: String;
  isAdmin?: String;
  isHidden?: String;
  picklistDatabaseName?: String;
  picklistType?: String;
  processorClass?: String;
}

export class CGroupSetting {
  name?: String;
  title?: String;
  description?: String;
  settings?: CSetting[];
}

export function getGroupSettings(groupSettingType: GroupSettingType, state: IAppState): CGroupSetting {
  let groupSettings: CGroupSetting[] = state.session.lookups ? state.session.lookups.systemSettings : [];
  return groupSettings.find(s => s.title && s.title.replace(' ', '').replace('-', '').toLowerCase() === GroupSettingType[groupSettingType].toLowerCase());
}

export function getSetting(groupSettingType: GroupSettingType, settingName: String, state: IAppState): CSetting {
  let groupSetting = getGroupSettings(groupSettingType, state);
  return groupSetting ? groupSetting.settings.find(s => s.name.toLowerCase() === settingName.toLowerCase()) : undefined;
}
