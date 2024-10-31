import { Component } from '@angular/core';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { System } from '../models/system';
import { CommonModule } from '@angular/common';
import { AccountService } from '../services/account.service';
import { SystemService } from '../services/system.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-system',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingComponent],
  templateUrl: './system.component.html',
  styleUrl: './system.component.css'
})
export class SystemComponent {
  parameters: number[] = [];
  roots: number[] | null = [];
  postSystemForm: FormGroup;
  postParametersForm: FormGroup;
  isPostSystemFormSubmitted: boolean = false;
  rows: number = 3;
  postRowsForm: FormGroup;
  isPostParametersSubmitted: boolean = false;
  systems: System[] = [];
  putSystemForm: FormGroup;
  //new
  loading: boolean = false;

  constructor(private systemService: SystemService, private accountService: AccountService) {
    this.postSystemForm = new FormGroup({
      parameters: new FormArray([])
    });

    this.postParametersForm = new FormGroup({
      parameters: new FormArray([]),
      roots: new FormArray([]),
    });

    this.postRowsForm = new FormGroup({
      rows: new FormControl(null)
    });

    this.putSystemForm = new FormGroup({
      systems: new FormArray([])
    })
  }

  get postRows_RowsControl(): any {
    return this.postRowsForm.controls['rows'];
  }
  get postParametersFormArray(): FormArray {
    return this.postParametersForm.get("parameters") as FormArray;
  }

  get postRootsFormArray(): FormArray {
    return this.postParametersForm.get("roots") as FormArray;
  }

  public resizePostForm(): void {
    this.rows = this.postRows_RowsControl.value;

    //console.log(`start this.parameters: ${this.parameters}`);

    if (this.parameters.length < this.rows) {
      let length = this.parameters.length;
      for (var i = length; i < this.rows; i++) {
        this.parameters.push(0);
      }
    }
    else {
      while (this.parameters.length != this.rows) {
        this.parameters.pop();
      }
    }

    //console.log(this.parameters);

    this.postParametersFormArray.clear();
    this.parameters.forEach((param) => {
      this.postParametersFormArray.push(new FormGroup({
        value: new FormControl(param, [Validators.required])
      }));
    });

    //console.log(`end this.parameters ${this.parameters}`);
  }

  public async postParametersSubmitted() {
    this.parameters = [];
    //new
    this.loading = true;

    //console.log(`this.postParametersFormArray.length --- ${this.postParametersFormArray.length}`);
    for (let i = 0; i < this.postParametersFormArray.length; i++) {
      let element = this.postParametersFormArray.at(i).value.value as number;
      this.parameters.push(element);
    }

    for (let i = 0; i < this.postParametersFormArray.length; i++) {
      this.postParametersFormArray.controls[i].reset(this.postParametersFormArray.controls[i].value);
    }

    console.log(this.parameters);

    let system = new System(null, this.parameters, null);

    this.postSystemSubmitted(system);

    
  }

  get putSystemFormArray(): FormArray {
    return this.putSystemForm.get("systems") as FormArray;
  }

  loadSystems() {
    this.systemService.getSystems().subscribe({
      next: (response: System[]) => {
        this.systems = response;

        this.systems.forEach(system => {
          this.putSystemFormArray.push(new FormGroup({
            id: new FormControl(system.id, [Validators.required]),
            name: new FormControl({ value: system.parameters, disabled: true }, [Validators.required])
          }));
        })
      },

      error: (error: any) => {
        console.log(error);
      },

      complete: () => { }
    });
  }
  ngOnInit() {
    this.loadSystems();
  }

  // removed postCity_NameControl

  public postSystemSubmitted(system: System) {
    this.isPostSystemFormSubmitted = true;
    //this.postSystemForm.value
    this.systemService.postSystem(system).subscribe({
      next: (response: System) => {
        this.roots = response.roots;
        console.log("this.roots: " + this.roots);

        this.postRootsFormArray.clear();

        if (this.roots != null) {
          this.roots.forEach((param) => {
            this.postRootsFormArray.push(new FormGroup({
              value: new FormControl(param, [Validators.required])
            }));
          });
        }

        //new
        this.loading = false;

        //this.loadSystems();
        //this.cities.push(response);
      },

      error: (error: any) => {
        console.log(error);
      },

      complete: () => { }
    });
  }

  /*public editClicked(System: System) {
    this.editId = System.id;
  }*/

  /*public updateClicked(i: number) {
    this.systemService.putSystem(this.putSystemFormArray.controls[i].value).subscribe({
      next: (response: string) => {
        console.log(response);
        console.log(this.putSystemFormArray.controls[i].value);

        this.editId = null;
        this.putSystemFormArray.controls[i].reset(this.putSystemFormArray.controls[i].value);
      },
      error: (error: any) => {
        console.log(error)
      },
      complete: () => {

      }
    })
  }*/

  public deleteClicked(System: System, i: number): void {
    if (confirm(`Are you sure to delete this System: ${System.parameters}?`)) {
      this.systemService.deleteSystem(System.id).subscribe({
        next: (response: string) => {
          console.log(response);
          //this.editId = null;

          this.putSystemFormArray.removeAt(i);
          this.systems.splice(i, 1);
        },
        error: (error: any) => {
          console.log(error);
        },
        complete: () => { }

      })
    }
  }

  refreshClicked(): void {
    this.accountService.postGenerateNewToken().subscribe({
      next: (response: any) => {
        localStorage["token"] = response.token;
        localStorage["refreshToken"] = response.refreshToken;

      },
      error: (error: any) => {
        console.log(error);
      },
      complete: () => { }
    });
  }
}