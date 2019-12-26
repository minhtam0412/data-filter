import {Guid} from 'guid-typescript';

export class ReportColumnView {
  Id? = Guid;
  UserId? = -1;
  ColumnCode?: string;
  CreatedDate?: Date;
  CreatedBy?: number;
  ModifyDate?: Date;
  ModifyBy?: number;
  ViewType?: number;
  ColumnName?: string;

  // custom properties
  SortOrder: number;
}
