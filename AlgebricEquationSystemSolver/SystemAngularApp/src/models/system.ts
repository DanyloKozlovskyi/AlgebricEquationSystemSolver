export class System {
  id: string | null = null;
  parameters: number[] | null = null;
  roots: number[] | null = null;

  constructor(id: string | null = null, parameters: number[] | null = null,
    roots: number[] | null = null) {
    this.id = id;
    this.parameters = parameters;
    this.roots = roots;
  }
}
