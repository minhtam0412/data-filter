import {Component, OnInit} from '@angular/core';
import {AuthenService} from '../authen.service';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent implements OnInit {

  constructor(private authenService: AuthenService) {
  }

  ngOnInit() {
    console.log('Logout');
    this.authenService.logout();
  }
}
