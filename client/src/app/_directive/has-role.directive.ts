import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]' //*appHasRole
})
export class HasRoleDirective implements OnInit{
  //acces to parameters for pass string into selector appHasRole = '["Admin"]'
  @Input() appHasRole!: string[];
  user!: User;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
      private accountService: AccountService) {
        //acces to currentUser and get him
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
          this.user = user;
        })
       }

  ngOnInit(): void {
    //clear view if no roles
    if(!this.user?.roles || this.user == null){
      this.viewContainerRef.clear();
      return;
    }

    //callback function on each element inside roles, returns true
    if(this.user?.roles.some(r => this.appHasRole.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    }else{
      this.viewContainerRef.clear();
    }
  }

}
