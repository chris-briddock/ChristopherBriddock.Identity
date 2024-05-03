import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [HttpClientModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  constructor(private authService: AuthService) { }
  email: string = '';
  password: string = '';

  onSubmit(): void {
    this.authService.login(this.email, this.password).subscribe({
      next: (response: LoginResponse) => {
        // Handle the response data here
        console.log('Access Token:', response.accessToken);
        console.log('Refresh Token:', response.refreshToken);
        console.log('Expires:', response.expires);
      },
      error: (error) => {
        // Handle any errors
        console.error('Error:', error);
      }
    });
  }
}
