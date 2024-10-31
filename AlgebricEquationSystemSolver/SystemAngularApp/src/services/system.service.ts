import { Injectable } from '@angular/core';
import { System } from "../models/system";
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from "rxjs";

const API_BASE_URL: string = "https://localhost:7220/api/";

@Injectable({
  providedIn: 'root'
})

export class SystemService {
  constructor(private httpClient: HttpClient) {

  }
  public getSystems(): Observable<System[]> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", `Bearer ${localStorage['token']}`);
    return this.httpClient.get<System[]>(`${API_BASE_URL}algebricEquationSystem`, { headers: headers });
  }
  public postSystem(System: System): Observable<System> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", `Bearer ${localStorage['token']}`);
    return this.httpClient.post<System>(`${API_BASE_URL}algebricEquationSystem`, System, { headers: headers });
  }
  public putSystem(System: System): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", `Bearer ${localStorage['token']}`);

    return this.httpClient.put<string>(`${API_BASE_URL}algebricEquationSystem/${System.id}`, System, { headers: headers });
  }
  public deleteSystem(id: string | null): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", `Bearer ${localStorage['token']}`);

    return this.httpClient.delete<string>(`${API_BASE_URL}algebricEquationSystem/${id}`, { headers: headers });
  }
}
