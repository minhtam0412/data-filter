/**
 * Usage: dateString | localDate:'format'
 **/

import { Pipe, PipeTransform } from '@angular/core';
import { formatDate } from '@angular/common';
import { SessionService } from '../session-service.service';

@Pipe({
  name: 'localDate'
})
export class LocalDatePipe implements PipeTransform {
  constructor(private session: SessionService) {}

  transform(value: any, format?: string) {
    if (!value) {
      return '';
    }
    if (!format) {
      format = 'short';
    }

    return formatDate(value, format, this.session.locale);
  }
}
