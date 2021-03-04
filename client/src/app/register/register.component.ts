import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister = new EventEmitter();

  model:any={};

  constructor(private accounService: AccountService) { }

  ngOnInit(): void {
  }

  register(){
    this.accounService.register(this.model).subscribe(response=>{
      console.log(response);
      this.cancle();
    }, error =>{
      console.log(error);
    })
  }

  cancle(){
    this.cancelRegister.emit(false);
  }
}
