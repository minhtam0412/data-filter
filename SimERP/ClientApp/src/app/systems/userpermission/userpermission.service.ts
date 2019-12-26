import {Inject, Injectable} from '@angular/core';
import {AuthenParams} from '../../common/commomodel/AuthenParams';
import {ReqListSearch} from '../../common/commomodel/ReqListSearch';
import {ReqListDelete} from '../../common/commomodel/ReqListDelete';
import {ReqListAdd} from '../../common/commomodel/ReqListAdd';
import {ReqListUpdateSortOrder} from '../../common/commomodel/ReqListUpdateSortOrder';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {ResponeResult} from '../../common/commomodel/ResponeResult';
import {UserRoleList} from './model/userpermission';
import { ROOT_URL } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class UserpermissionService {

  AuthenParams = new AuthenParams();
  SearchParams = new ReqListSearch();
  DelParams = new ReqListDelete();
  InsertParams = new ReqListAdd();
  ReqListUpdateSortOrder = new ReqListUpdateSortOrder();

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseURL: string) { 
    this.baseURL = ROOT_URL;
  }

  getListUser() {

    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;
    const jsonString = JSON.stringify(this.SearchParams);

    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/user/getlistuser', jsonString, { headers });

  }

  getRoleUser( userID?: number) {

    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;
    this.SearchParams.SearchString = userID == null ? null : userID.toString();
    const jsonString = JSON.stringify(this.SearchParams);

    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/user/getroleuser', jsonString, { headers });

  }

  getRoleList( userID?: number) {

    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;
    this.SearchParams.SearchString = userID == null ? null : userID.toString();
    const jsonString = JSON.stringify(this.SearchParams);

    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/user/getrolelist', jsonString, { headers });

  }

  Insert(obj: UserRoleList[], LstPermission:string, UserId: number) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.InsertParams.AuthenParams = this.AuthenParams;
    this.InsertParams.RowData = obj;

    var param = { "datasave": this.InsertParams, "lstpermission": LstPermission, "userid": UserId };
    const jsonString = JSON.stringify(param);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/user/saveuserrole', jsonString, { headers });
  }

  LoadPageListRole( moduleID?: number, userID?: number) {

    var par_moduleID = moduleID == -1 ? null : moduleID;

    var param = { "moduleID": par_moduleID, "userID": userID };
    const jsonString = JSON.stringify(param);

    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/user/loadpagelistrole', jsonString, { headers });
  }
}
