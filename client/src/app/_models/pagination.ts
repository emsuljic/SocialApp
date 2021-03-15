export interface Pagination{
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages:number;
}

export class PaginatedResult<T>{
    //list of memebers are gona be stored in result
    result!: T;
    //pagination information are stored in pagination
    pagination!: Pagination;
}