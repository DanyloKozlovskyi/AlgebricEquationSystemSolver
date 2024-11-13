import { Injectable } from '@angular/core';
import { System } from "../models/system";
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from "rxjs";

const API_BASE_URL: string = "https://localhost:7221/api/";

@Injectable({
  providedIn: 'root'
})

export class SystemService {
  constructor(private httpClient: HttpClient) {

  }
  public getSystems(): Observable<System[]> {
    console.log("Get systems " + "localStorage['token']: " + localStorage['token']);
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", `Bearer ${localStorage['token']}`);
    return this.httpClient.get<System[]>(`${API_BASE_URL}tasks`, { headers: headers });
  }
  public postSystem(System: System): Observable<string | null> {
    //console.log(System);
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", `Bearer ${localStorage['token']}`);
    return this.httpClient.post<string | null>(`${API_BASE_URL}tasks`, System, { headers: headers });
  }
  public putSystem(System: System): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", `Bearer ${localStorage['token']}`);

    return this.httpClient.put<string>(`${API_BASE_URL}tasks/${System.id}`, System, { headers: headers });
  }
  public deleteSystem(id: string | null): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", `Bearer ${localStorage['token']}`);

    return this.httpClient.delete<string>(`${API_BASE_URL}tasks/${id}`, { headers: headers });
  }
  public terminateSystem(id: string | null): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", `Bearer ${localStorage['token']}`);
    console.log("terminateSystem " + localStorage['token']);
    return this.httpClient.delete<string>(`${API_BASE_URL}tasks/terminate/${id}`, { headers: headers });
  }
  public getTaskProgress(id: string | null): Observable<boolean> {
    console.log("localStorage['token']: " + localStorage['token']);
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", `Bearer ${localStorage['token']}`);
    return this.httpClient.get<boolean>(`${API_BASE_URL}tasks/${id}/status`, { headers: headers });
  }
}
