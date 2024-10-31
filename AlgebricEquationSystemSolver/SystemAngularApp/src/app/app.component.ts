import { Component } from '@angular/core';
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule, HttpClientModule, ReactiveFormsModule, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  constructor(public accountService: AccountService, private router: Router) {

  }

  logOutClicked() {
    this.accountService.getLogout().subscribe({
      next: (response: string) => {
        console.log(response);
        this.accountService.currentUserName = null;

        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.log(error);
      },
      complete: () => {

      }
    });
  }
}
