import { TypedRecord } from 'typed-immutable-record';

export interface IValidationRuleData {
  name: string;
  min: number;
  max: number;
  maxLength: number;
  error: string;
  defaultValue: string;
  parameters: any[];
}

export interface ICustomTableData {
  config?: any;
  rows: any[];
};

export class CCustomTableData implements ICustomTableData {
  config: any;
  rows: any[];
  constructor(config: any = {}, rows: any[] = []) {
    this.config = config;
    this.rows = rows;
  }
}

export interface IConfiguration {
  customTables: any;
  formGroups: any;
};

export interface IConfigurationRecord extends TypedRecord<IConfigurationRecord>, IConfiguration {
};
