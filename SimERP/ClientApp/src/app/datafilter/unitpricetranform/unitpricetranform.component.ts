import {AfterViewInit, Component, Input, OnInit} from '@angular/core';
import {ReportTotal} from '../model/reporttotal';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-unitpricetranform',
  templateUrl: './unitpricetranform.component.html',
  styleUrls: ['./unitpricetranform.component.css']
})
export class UnitpricetranformComponent implements OnInit, AfterViewInit {
  @Input() lstDataResult: ReportTotal[] = [];
  @Input() lstDataResultTmp: ReportTotal[] = [];

  constructor(private activeModal: NgbActiveModal) {

  }

  ngOnInit() {
  }

  closeDialog() {
    this.activeModal.close();
  }

  updateData() {
    this.lstDataResultTmp.forEach(valueResult => {
      this.lstDataResult.forEach(valueOriginal => {
        if (valueResult.id === valueOriginal.id) {
          valueOriginal.DonGiaUSD = valueResult.DonGiaUSD;
        }
      });
    });
    this.activeModal.close(this.lstDataResult);
  }

  ngAfterViewInit(): void {
    this.lstDataResult.forEach(value => {
      this.lstDataResultTmp.push(Object.assign({}, value));
    });

    this.lstDataResultTmp.forEach(value => {
      value.DonGiaUSD = value.TriGiaUsd / value.SoLuongQuyDoi;
    });
  }
}
