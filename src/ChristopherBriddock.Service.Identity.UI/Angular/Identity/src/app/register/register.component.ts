import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, HttpClientModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})

export class RegisterComponent {
  constructor(private http: HttpClient) { }
  email: string = '';
  password: string = '';

  onSubmit(): void {
    const body: any = { email: this.email, password: this.password };
    console.log(body);
    this.http.post<any>('https://localhost:5119', body).subscribe(data => {
      
    });
  }
}
