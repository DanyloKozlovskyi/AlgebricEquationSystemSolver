import { Subject } from "rxjs";

export class System {
  id: string | null = null;
  parameters: number[] | null = null;
  roots: number[] | null = null;
  isCompleted: boolean = false;
  destroy$: Subject<void> = new Subject<void>();

  constructor(id: string | null = null, parameters: number[] | null = null,
    roots: number[] | null = null, isCompleted: boolean = false) {
    this.id = id;
    this.parameters = parameters;
    this.roots = roots;
    this.isCompleted = isCompleted;
  }
}
