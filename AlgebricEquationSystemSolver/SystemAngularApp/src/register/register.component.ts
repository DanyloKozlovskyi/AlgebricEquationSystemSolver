import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { RegisterUser } from '../models/register-user';
import { CommonModule } from '@angular/common';
//import { CompareValidation } from '../validators/custom-validators';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isRegisterFormSubmitted: boolean = false;
  isRegisterValid: boolean = true;

  constructor(private accountService: AccountService, private router: Router) {
    this.registerForm = new FormGroup({
      personName: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      phoneNumber: new FormControl(null, [Validators.required]),
      password: new FormControl(null, [Validators.required]),
      confirmPassword: new FormControl(null, [Validators.required])
    }, {
      //validators: [CompareValidation("password", "confirmPassword")]
    })
  }

  get register_nameControl(): any {
    return this.registerForm.controls['personName'];
  }
  get register_emailControl(): any {
    return this.registerForm.controls['email'];
  }
  get register_phoneControl(): any {
    return this.registerForm.controls['phoneNumber'];
  }
  get register_passwordControl(): any {
    return this.registerForm.controls['password'];
  }
  get register_confirmPasswordControl(): any {
    return this.registerForm.controls['confirmPassword'];
  }

  registerSubmitted() {
    this.isRegisterFormSubmitted = true;
    if (this.registerForm.valid) {

      this.accountService.postRegister(this.registerForm.value).subscribe({
        next: (response: any) => {
          console.log(response);
          this.isRegisterValid = true;

          this.accountService.currentUserName = response.email;
          this.isRegisterFormSubmitted = false;
          localStorage["token"] = response.token;
          localStorage["refreshToken"] = response.refreshToken;

          this.router.navigate(['/system']);

          this.registerForm.reset();

        },
        error: (error: any) => {
          this.isRegisterValid = false;
          console.log(error);
        },
        complete: () => { }
      });
    }
  }
}
