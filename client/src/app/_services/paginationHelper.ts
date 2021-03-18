//set members
  //pass our paramteres in return method
  //when we use HTTP get normally, it's give us response.body , but..->
  //when we're observing the response, and use it to pass parameters that we created, 

import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_models/pagination";

  //then we get the full response back, don't get body response so we need to do it ourselves
export function getPaginatedResult<T>(url: string, params: HttpParams, http: HttpClient) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>()!;
    return http.get<T>(url, { observe: 'response', params }).pipe(
      //use map, so we can hold the response
      map(response => {
        //our members array is gona contained inside reponse.body
        paginatedResult.result = response.body;
        //check pagination headers that not equal null
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination') || '{}');
        }
        return paginatedResult ;
      })
    );
  }

  //get pagination headers function
export function getPaginationHeader(pageNumber: number, pageSize: number) {
    //this gives us to serialize up parameters
    let params = new HttpParams();

    //query string; so page needs to be toString()
    params = params.append('pageNumber', pageNumber!.toString());
    params = params.append('pageSize', pageSize!.toString());

    return params;
  }