export interface IColumnConfig {
  name: string;
  label?: string;
  type?: string;
  idField?: boolean;
  lookup?: any[];
}

/**
 * Returns the view compatible column list from the column configuration list from the server
 * @param columnConfig The list of column configuration data from server
 */
export function getViewColumns(columnConfig: IColumnConfig[]): any[] {
  let viewColumns = [];
  columnConfig.forEach(cc => {
    let viewColumn: any = {};
    viewColumn.dataField = cc.name;
    viewColumn.caption = cc.label ? cc.label : cc.name;
    if (cc.lookup && cc.lookup.length > 0) {
      let keys = Object.keys(cc.lookup[0]);
      viewColumn.lookup = {
        dataSource: cc.lookup,
        valueExpr: keys[0],
        displayExpr: keys[1]
      };
    }
  });
  return viewColumns;
}
