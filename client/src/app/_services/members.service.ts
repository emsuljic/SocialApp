import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  
  constructor(private http: HttpClient) { }

  getMembers(){
    //check if there is members, and return them from service as obesrvable of(this.member)
    if(this.members.length > 0) return of(this.members);
    //set members
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    )
  }

  getMember(username: string){
    //get member from service Member
    //finding member with the sam eusername that we parsing throw parameter
    const member = this.members.find(x => x.username === username);
    //if member in find() is not undefined 
    if(member !== undefined) return of(member);
    //if we don't have a member we call him from API call
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        //get member from the service, find index of member
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }
}
