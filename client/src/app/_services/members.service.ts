import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, pipe } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  //use object Map() to store value in key-value format
  memberCache = new Map();
  user!: User;
  userParams!: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  //pass pagination parameter
  getMembers(userParams: UserParams) {
    //check if we have in our cache response for particular query
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    //if we have response for particular key, pass response
    if (response) {
      return of(response);
    }

    let params = getPaginationHeader(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    //when get results back use map funciton and transform data
    //idea- we go to our API and getMembers if we dont have it in cache, but if we have then in cache we will retriev this
    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(map(response => {
        //.set(key, value)
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }))

  }

  getMember(username: string) {
    //finding individual member inside memberCache
    //get all values from memberCache
    //.reduce() reduce our array in to something else; 
    //want the result of each array in single array that we can search, and find first member that matches a condition
    const member = [...this.memberCache.values()]
      //what to do with each array 
      //we call this function on each element in the array, we're gona get the result which contains everything what's in cache (elem.result)
      //and then we+re gona concat all that in the one arr->/array/ that we specify, which starts with nothing -> []
      .reduce((arr, elem) => arr.concat(elem.result), [])
      //look for first instance of member.username, and returns to us
      .find((member: Member) => member.username === username);

    //check if we already have member in previous queries, avoiding duplicate
    if (member) {
      return of(member);
    }

    //if we don't have a member we call him from API call
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        //get member from the service, find index of member
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }


  addLike(username: string){
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber: number , pageSize: number ){
    let params = getPaginationHeader(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params, this.http);
  }

}
