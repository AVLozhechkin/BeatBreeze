import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatTime',
  standalone: true,
})
export class FormatTimePipe implements PipeTransform {
  transform(seconds: number): string {
    const formattedminutes = Math.floor(seconds / 60) || 0;

    const formattedSeconds = Math.floor(seconds - formattedminutes * 60 || 0);

    return `${formattedminutes}:${
      formattedSeconds < 10 ? 0 : ''
    }${formattedSeconds}`;
  }
}
