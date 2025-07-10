using EmployeeApi.Models;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
    {
        return Ok(await _employeeService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> Get(Guid id)
    {
        var emp = await _employeeService.GetByIdAsync(id);
        if (emp == null) return NotFound();
        return Ok(emp);
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> Create([FromBody] Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _employeeService.CreateAsync(employee);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _employeeService.UpdateAsync(id, employee);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _employeeService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
