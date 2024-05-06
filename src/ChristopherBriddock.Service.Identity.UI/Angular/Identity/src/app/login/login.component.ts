import { Component } from '@angular/core';
import { LoginResponse } from '../../models/LoginResponse';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, HttpClientModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email: string = '';
  password: string = '';

  constructor(private authService: AuthService) { }

  onSubmit(): void {
    this.authService.login(this.email, this.password).subscribe({
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
