import { TypedRecord } from 'typed-immutable-record';

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
