import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'size',
  standalone: true,
})
export class SizePipe implements PipeTransform {
  transform(value: number, ...args: unknown[]): string {
    let sizeInMb = Math.round((value / 1024 / 1024) * 100) / 100;
    return `${sizeInMb} mb`;
  }
}
