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

    let params = this.getPaginationHeader(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    //when get results back use map funciton and transform data
    //idea- we go to our API and getMembers if we dont have it in cache, but if we have then in cache we will retriev this
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)
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


  //set members
  //pass our paramteres in return method
  //when we use HTTP get normally, it's give us response.body , but..->
  //when we're observing the response, and use it to pass parameters that we created, 
  //then we get the full response back, don't get body response so we need to do it ourselves
  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>()!;
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      //use map, so we can hold the response
      map(response => {
        //our members array is gona contained inside reponse.body
        paginatedResult.result = response.body;
        //check pagination headers that not equal null
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination') || '{}');
        }
        return paginatedResult;
      })
    );
  }

  //get pagination headers function
  private getPaginationHeader(pageNumber: number, pageSize: number) {
    //this gives us to serialize up parameters
    let params = new HttpParams();

    //query string; so page needs to be toString()
    params = params.append('pageNumber', pageNumber!.toString());
    params = params.append('pageSize', pageSize!.toString());

    return params;
  }
}
