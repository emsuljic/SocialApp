import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {

  @Input() label?: string;
  @Input() maxDate?: Date;
  //partial: every single property in this type is going to be optional
  //without partial we have to provide every single possible configuration option that we put inside 
  bsConfig?: Partial<BsDatepickerConfig>;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig={
      containerClass: 'theme-red', //theme 
      dateInputFormat: 'DD MMMM YYYY',//how we want this display inside datePicker
    }
   }

  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {

  }
  registerOnTouched(fn: any): void {
  }

  

}
