import {Inject, Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {CustomerDetail} from './model/customerdetail';
import {ReqListDelete} from '../../common/commomodel/ReqListDelete';
import {ResponeResult} from '../../common/commomodel/ResponeResult';
import {ReqListSearch} from '../../common/commomodel/ReqListSearch';
import {AuthenParams} from '../../common/commomodel/AuthenParams';
import {ReqListAdd} from '../../common/commomodel/ReqListAdd';
import {ReqListUpdateSortOrder} from '../../common/commomodel/ReqListUpdateSortOrder';
import { ROOT_URL } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CustomerdetailService {

  AuthenParams = new AuthenParams();
  SearchParams = new ReqListSearch();
  DelParams = new ReqListDelete();
  InsertParams = new ReqListAdd();
  ReqListUpdateSortOrder = new ReqListUpdateSortOrder();

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private readonly baseURL: string,) {
    this.baseURL = ROOT_URL;
  }

  getData(searchString?: string, areaId?: number, startRow?: number, maxRow?: number) {

    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;
    this.SearchParams.MaxRow = maxRow;
    this.SearchParams.StartRow = startRow;
    this.SearchParams.SearchString = searchString;
    const par_areaId = areaId === -1 ? null : areaId;

    const param = {'dataserach': this.SearchParams, 'areaId': par_areaId};
    const jsonString = JSON.stringify(param);

    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/list/customerdetail', jsonString, {headers});
  }

  Insert(obj: CustomerDetail, isNew: boolean) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.InsertParams.AuthenParams = this.AuthenParams;
    this.InsertParams.RowData = obj;
    this.InsertParams.IsNew = isNew;

    const jsonString = JSON.stringify(this.InsertParams);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/list/savecustomerdetail', jsonString, {headers});
  }

  Delete(Id: any) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.DelParams.AuthenParams = this.AuthenParams;
    this.DelParams.ID = Id;
    const jsonString = JSON.stringify(this.DelParams);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/list/deletecustomerdetail', jsonString, {headers});
  }

  Sort(UpID: number, DowID: number) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.ReqListUpdateSortOrder.AuthenParams = this.AuthenParams;
    this.ReqListUpdateSortOrder.UpID = UpID;
    this.ReqListUpdateSortOrder.DownID = DowID;

    const jsonString = JSON.stringify(this.ReqListUpdateSortOrder);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/list/updateSortOrderCustomerDetail', jsonString, {headers});
  }

  GetProductCache() {

    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;

    const jsonString = JSON.stringify(this.SearchParams);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/list/productcache', jsonString, {headers});
  }
}
