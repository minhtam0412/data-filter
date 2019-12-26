import { LocalNumberPipe } from '../locale/pipes/local-number-pipe.service';
import { SessionService } from '../locale/session-service.service';

declare var jquery: any;
declare var $: any;
export class Globalfunctions {
  static localNumberPipe: LocalNumberPipe;
  static session: SessionService = new SessionService();
  
  constructor() {
  }

  static openInNewTab(url) {
    const win = window.open(url, '_blank');
    win.focus();
  }

  static debugBase64(base64URL) {
    const win = window.open();
    win.document.title = 'SIM ERP';
    win.document.write('<iframe src="' + base64URL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; ' +
      'width:100%; height:100%;" allowfullscreen></iframe>');
  }

  static checkIsImageFile(file: File) {
    const mimeType = file.type;
    return mimeType.match(/image\/*/) != null;
  }

  static checkIsImageByExtension(data) {
    const allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif)$/i;
    if (!allowedExtensions.exec(data)) {
      return false;
    }
    return true;
  }

  static isNumberKey(evt: any) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode == 59 || charCode == 46)
      return true;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
      return false;
    return true;
  }

  static formatNumber(IdName: string, value: number, formar: string = '.0-0') {
    Globalfunctions.localNumberPipe = new LocalNumberPipe(this.session);
    $("#" + IdName).val(Globalfunctions.localNumberPipe.transform(value, formar));
  }

}
