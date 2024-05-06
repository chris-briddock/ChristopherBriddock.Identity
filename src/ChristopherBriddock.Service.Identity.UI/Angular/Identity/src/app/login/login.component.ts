import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoginResponse } from '../../models/LoginResponse';
import { HttpClientModule } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [HttpClientModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  constructor() { }
  email: string = '';
  password: string = '';

  onSubmit(authService: AuthService): void {
    authService.login(this.email, this.password).subscribe({
      next: (response: LoginResponse) => {
        console.log('Access Token:', response.accessToken);
        console.log('Refresh Token:', response.refreshToken);
        console.log('Expires:', response.expires);
      },
      error: (error) => {
        console.error('Error:', error);
      }
    });
  }
}
