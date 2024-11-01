import { Routes } from '@angular/router';
import { SystemComponent } from '../system/system.component';
import { LoginComponent } from '../login/login.component';
import { RegisterComponent } from '../register/register.component';
import { AppComponent } from './app.component';

export const routes: Routes = [
  { path: "system", component: SystemComponent },
  { path: "register", component: RegisterComponent },
  { path: "login", component: LoginComponent },
  { path: "logout", component: AppComponent }
];
