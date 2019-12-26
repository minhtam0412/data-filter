import {Pipe, PipeTransform} from '@angular/core';

const padding = '000000';

@Pipe({
  name: 'myformatnumber'
})
export class FormatnumberPipe implements PipeTransform {
  private prefix: string;
  private decimal_separator: string;
  private thousands_separator: string;
  private suffix: string;
  public fraction: number;

  constructor() {
    this.prefix = '';
    this.suffix = '';
    this.decimal_separator = '.';
    this.thousands_separator = ',';
  }

  transform(value: string, fractionSize: number = 0): string {
    if (value == null || value.length === 0) {
      return '';
    }
    if (parseFloat(value) % 1 !== 0) {
      fractionSize = this.fraction;
    }
    let [integer, fraction = ''] = (parseFloat(value).toString() || '').toString().split('.');

    // nếu phần thập phân = 0 thì bỏ qua luôn, không cần quan tâm phải hiển thị bao nhiu số lẻ nữa
    if (Number(fraction) === 0) {
      fractionSize = 0;
    }

    fraction = fractionSize > 0
      ? this.decimal_separator + (fraction).substring(0, fractionSize) : '';
    integer = integer.replace(/\B(?=(\d{3})+(?!\d))/g, this.thousands_separator);
    if (isNaN(parseFloat(integer))) {
      integer = '';
    }
    return this.prefix + integer + fraction + this.suffix;
  }

  parse(value: string, fractionSize: number = 0): string {
    if (value == null || value.length === 0) {
      return '';
    }
    if (this.fraction != null) {
      fractionSize = this.fraction || 0;
    }

    let [integer, fraction = ''] = (value || '').replace(this.prefix, '')
      .replace(this.suffix, '')
      .split(this.decimal_separator);

    integer = integer.replace(new RegExp(this.thousands_separator, 'g'), '');

    if (fractionSize > 0) {
      fraction = parseInt(fraction, 10) > 0 && fractionSize > 0
        ? this.decimal_separator + (fraction).substring(0, fractionSize)
        : '';
    }
    return integer + fraction;
  }

}
