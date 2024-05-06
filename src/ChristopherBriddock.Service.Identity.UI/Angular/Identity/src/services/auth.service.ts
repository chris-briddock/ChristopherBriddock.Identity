import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginRequest } from '../models/LoginRequest';
import { LoginResponse } from '../models/LoginResponse';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }

  // Method to handle the login request
  public login(email: string, password: string) {
    const body: LoginRequest = { email, password };
    return this.http.post<LoginResponse>('https://localhost:5119/authorize', body);
  }
}
