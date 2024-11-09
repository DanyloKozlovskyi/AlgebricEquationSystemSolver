import { Component } from '@angular/core';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { System } from '../models/system';
import { CommonModule } from '@angular/common';
import { AccountService } from '../services/account.service';
import { SystemService } from '../services/system.service';
import { LoadingComponent } from '../loading/loading.component';
import { DisableControlDirective } from '../directives/disabled-control.directive';
import { v4 as uuid } from 'uuid'

@Component({
  selector: 'app-system',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingComponent, DisableControlDirective],
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
  loading: boolean = false;

  editId: string | null = null;

  tasksInRun: number = 0;
  maxTasks: number = 2;


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
    console.log(this.tasksInRun)
    if (this.tasksInRun >= this.maxTasks) {
      alert(`maximum number of tasks was reached, unable to start more tasks`);
      console.log("alert reached");
      return;
    }
    this.tasksInRun++;
    this.parameters = [];
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

    ////////

    let system = new System(uuid(), this.parameters, null);

    this.postSystemSubmitted(system);
  }
  
  get putSystemFormArray(): FormArray {
    return this.putSystemForm.get("systems") as FormArray;
  }

  loadSystems() {
    this.systemService.getSystems().subscribe({
      next: (response: System[]) => {
        this.systems = response;

        console.log(this.systems);
        console.log(this.putSystemFormArray);

        var parameters = [];
        for (var i = 0; i < this.systems.length; i++) {
          var str = '';
          var currentLength = this.systems.at(i)?.parameters?.length;
          if (currentLength != undefined) {
            for (var j = 0; j < currentLength; j++) {
               
            }
          }
        }

        this.putSystemFormArray.clear();

        this.systems.forEach(system => {
          this.putSystemFormArray.push(new FormGroup({
            id: new FormControl(system.id, [Validators.required]),
            parameters: new FormArray([]),
          }));
        });

        console.log(this.systems);

        this.systems.forEach((system: System, ind) => {
          system.parameters?.forEach((param) => {
            this.putSystem_ParametersControl(ind).push(new FormGroup({
              value: new FormControl(param, [Validators.required])
          }))});
        });
        console.log(this.putSystemFormArray);
      },

      error: (error: any) => {
        console.log(error);
      },

      complete: () => { }
    });
  }
  ngOnInit() {
    this.refreshClicked();
    this.loadSystems();
  }

  // removed postCity_NameControl
  public putSystem_ParametersControl(i: number): FormArray {
    let currentFormGroup = this.putSystemFormArray.controls[i] as FormGroup;
    return currentFormGroup.controls['parameters'] as FormArray;
  }

  public postSystemSubmitted(system: System) {
    this.isPostSystemFormSubmitted = true;

    this.systems.push(system);
    
    this.putSystemFormArray.push(new FormGroup({
        id: new FormControl(system.id, [Validators.required]),
        parameters: new FormArray([]),
      }));
    
    system.parameters?.forEach((param) => {
      this.putSystem_ParametersControl(this.systems.length - 1).push(new FormGroup({
          value: new FormControl(param, [Validators.required])
        }))
    });

    //this.postSystemForm.value
    this.systemService.postSystem(system).subscribe({
      next: (response: System | null) => {
        if (response != null) {
          this.tasksInRun--;
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

          this.loading = false;

          this.loadSystems();
        }
        //this.cities.push(response);
      },

      error: (error: any) => {
        console.log(error);
      },

      complete: () => { }
    });
  }

  public editClicked(System: System) {
    this.editId = System.id;
  }

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

  public deleteClicked(system: System, i: number): void {
    if (confirm(`Are you sure to delete this System: ${system.parameters}?`)) {
      this.systemService.deleteSystem(system.id).subscribe({
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

  public terminateClicked(system: System, i: number): void {
    if (confirm(`Are you sure to terminate this System: ${system.parameters}?`)) {
      this.systemService.terminateSystem(system.id).subscribe({
        next: (response: string) => {
          console.log(`terminate response: ${response}`);
          this.tasksInRun--;
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

  public checkRoots(i: number): void {
    this.roots = this.systems.at(i)?.roots as number[];
    
    this.postRootsFormArray.clear();

    if (this.roots != null) {
      this.roots.forEach((param) => {
        this.postRootsFormArray.push(new FormGroup({
          value: new FormControl(param, [Validators.required])
        }));
      });
    }
  }
}
